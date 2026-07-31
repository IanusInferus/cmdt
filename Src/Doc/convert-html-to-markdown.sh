#!/usr/bin/env bash
set -euo pipefail

script_dir="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
lua_filter="$script_dir/word-html-cleanup.lua"
html_preprocessor="$script_dir/word-html-preprocess.pl"

for command_name in pandoc iconv perl; do
    if ! command -v "$command_name" >/dev/null 2>&1; then
        printf 'error: required command not found: %s\n' "$command_name" >&2
        exit 1
    fi
done

if [[ ! -f "$lua_filter" ]]; then
    printf 'error: cleanup filter not found: %s\n' "$lua_filter" >&2
    exit 1
fi

if [[ ! -f "$html_preprocessor" ]]; then
    printf 'error: HTML preprocessor not found: %s\n' \
        "$html_preprocessor" >&2
    exit 1
fi

detect_encoding() {
    local input_file="$1"
    local bom
    bom="$(od -An -tx1 -N3 "$input_file" | tr -d ' \n')"

    case "$bom" in
        fffe*)
            printf 'UTF-16LE'
            ;;
        efbbbf)
            printf 'UTF-8'
            ;;
        *)
            printf 'UTF-8'
            printf 'warning: no recognized BOM in %s; assuming UTF-8\n' \
                "$input_file" >&2
            ;;
    esac
}

converted=0
while IFS= read -r -d '' input_file; do
    output_file="${input_file%.htm}.md"
    temporary_file="${output_file}.tmp"
    encoding="$(detect_encoding "$input_file")"

    rm -f -- "$temporary_file"
    trap 'rm -f -- "$temporary_file"' EXIT

    iconv -f "$encoding" -t UTF-8 "$input_file" |
        perl "$html_preprocessor" |
        pandoc \
            --from=html-native_divs \
            --to=gfm \
            --wrap=none \
            --lua-filter="$lua_filter" \
            --output="$temporary_file"

    mv -- "$temporary_file" "$output_file"
    trap - EXIT
    printf 'converted: %s -> %s (%s)\n' \
        "${input_file#"$script_dir"/}" \
        "${output_file#"$script_dir"/}" \
        "$encoding"
    ((converted += 1))
done < <(find "$script_dir" -type f -name '*.htm' -print0 | sort -z)

printf 'converted %d HTML files\n' "$converted"
