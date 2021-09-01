#!/usr/bin/env python3
from random import random
from sys import argv, stderr, stdin

DEFAULT_FADED_CHARACTER = ' '
DEFAULT_FADED_CHANCE    = 0.35

USAGE = '''\
Usage: ./faded.py [faded character] [faded chance]

faded character:
    a string (not necessarily a single character) to randomly replace
    characters with
faded chance:
    a decimal number between 0 and 1 inclusive representing the probability
    that any given character will be replaced with the faded character
'''

def main(
    faded_character=DEFAULT_FADED_CHARACTER,
    faded_chance=DEFAULT_FADED_CHANCE,
):
    faded_chance = float(faded_chance)

    for line in stdin:
        print(''.join([
            maybe_faded(character, faded_character, faded_chance)
            for character in line.rstrip()
        ]))

def maybe_faded(
    character,
    faded_character=DEFAULT_FADED_CHARACTER,
    faded_chance=DEFAULT_FADED_CHANCE,
):
    return faded_character if random() < faded_chance else character

if __name__ == '__main__':
    try:
        main(*argv[1:])
    except TypeError:
        stderr.write(USAGE)
