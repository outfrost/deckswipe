#!/usr/bin/python3

# Fixes indentation to keep tabs on empty lines
# Usage:
# ./fix-indent.py [<filename>, [<filename>, ...]]

import fileinput, re

for line in fileinput.input(inplace=True):
    if fileinput.isfirstline():
        indent = 0
    (line, num_subs) = re.subn(r"^\t*$", "\t" * indent, line)
    if num_subs == 0:
        indent += len(re.findall(r"{", line))
        indent -= len(re.findall(r"}", line))
    print(line, end='')
