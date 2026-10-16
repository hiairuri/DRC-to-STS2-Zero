import os, re, sys, json

SRC = r"D:\项目\dragonraja\sts2_work\silentfx_decomp\SilentFX.decompiled.cs"
OUT = r"D:\项目\dragonraja\sts2_work\silentfx_decomp\split"
os.makedirs(OUT, exist_ok=True)

with open(SRC, "r", encoding="utf-8") as f:
    lines = f.readlines()

# find namespace blocks at col 0
ns_re = re.compile(r"^namespace\s+([\w.]+)")
cls_re = re.compile(r"^(\t+)(?:\[[^\]]*\]\s*)?(public|internal|private|protected)?\s*(?:static\s+|sealed\s+|abstract\s+|partial\s+|readonly\s+)*class\s+(\w+)")
enum_re = re.compile(r"^(\t+)(?:\[[^\]]*\]\s*)?(?:public|internal|private|protected)?\s*enum\s+(\w+)")
struct_re = re.compile(r"^(\t+)(?:\[[^\]]*\]\s*)?(?:public|internal|private|protected)?\s*struct\s+(\w+)")
iface_re = re.compile(r"^(\t+)(?:\[[^\]]*\]\s*)?(?:public|internal|private|protected)?\s*interface\s+(\w+)")
delegate_re = re.compile(r"^(\t+)(?:\[[^\]]*\]\s*)?(?:public|internal|private|protected)?\s*delegate\s+[\w<>\[\],\s\.]+?\s(\w+)\s*\(")

def sanitize(name):
    return re.sub(r"[<>:\"\\/\|\?\*]", "_", name)

index = []  # {namespace, type, kind, file, lines, start, end}
cur_ns = None
cur_ns_start = None
# We'll collect segments: list of (ns, kind, typename, start_line_idx, indent)
segments = []
for i, line in enumerate(lines):
    m = ns_re.match(line)
    if m:
        cur_ns = m.group(1)
        continue
    if cur_ns is None:
        continue
    m = cls_re.match(line) or enum_re.match(line) or struct_re.match(line) or iface_re.match(line)
    if m:
        indent = len(m.group(1))
        # kind
        for rx, kind in ((cls_re, "class"), (enum_re, "enum"), (struct_re, "struct"), (iface_re, "interface"), (delegate_re, "delegate")):
            mm = rx.match(line)
            if mm:
                break
        typename = m.group(m.lastindex)
        segments.append({"ns": cur_ns, "kind": kind, "type": typename, "line": i, "indent": indent})

# close each segment at next segment with indent <= its indent
for idx, seg in enumerate(segments):
    end = len(lines)
    for j in range(idx + 1, len(segments)):
        if segments[j]["indent"] <= seg["indent"]:
            end = segments[j]["line"]
            break
    seg["end"] = end

# only write top-level (indent==1) segments; nested types stay with parent
written = 0
for seg in segments:
    if seg["indent"] != 1:
        continue
    fname = f"{sanitize(seg['ns'])}.{sanitize(seg['type'])}.cs"
    path = os.path.join(OUT, fname)
    body = lines[seg["line"]:seg["end"]]
    with open(path, "w", encoding="utf-8") as f:
        f.write(f"// namespace: {seg['ns']}\n// type: {seg['type']} ({seg['kind']})\n// source lines: {seg['line']+1}-{seg['end']}\n")
        f.writelines(body)
    index.append({
        "namespace": seg["ns"], "type": seg["type"], "kind": seg["kind"],
        "file": fname, "lines": seg["end"] - seg["line"],
        "src_start": seg["line"] + 1, "src_end": seg["end"],
    })
    written += 1

with open(os.path.join(OUT, "_index.json"), "w", encoding="utf-8") as f:
    json.dump(index, f, indent=1, ensure_ascii=False)

# summary
from collections import Counter
by_ns = Counter()
by_ns_lines = Counter()
for e in index:
    by_ns[e["namespace"]] += 1
    by_ns_lines[e["namespace"]] += e["lines"]
print(f"written files: {written}")
for ns in sorted(by_ns, key=lambda n: -by_ns_lines[n]):
    print(f"  {ns}: {by_ns[ns]} types, {by_ns_lines[ns]} lines")
print(f"total indexed lines: {sum(by_ns_lines.values())} / {len(lines)}")
