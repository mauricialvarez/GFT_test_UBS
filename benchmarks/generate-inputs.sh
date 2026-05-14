#!/usr/bin/env bash
set -euo pipefail

output_dir="${1:-"$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)/data"}"
mkdir -p "$output_dir"

sizes=(1000 10000 100000 1000000 10000000)
templates=(
  "2000000 Private 12/29/2025"
  "400000 Public 07/01/2020"
  "5000000 Public 01/02/2024"
  "3000000 Public 10/26/2023"
)

for size in "${sizes[@]}"; do
  path="$output_dir/input_${size}.txt"
  {
    printf '12/11/2020\n'
    printf '%s\n' "$size"

    for ((i = 0; i < size; i++)); do
      printf '%s\n' "${templates[$((i % ${#templates[@]}))]}"
    done
  } > "$path"
done
