#!/usr/bin/env bash
set -euo pipefail

SERVICE="${1:?service is required}"
SERVICE_PATH="${2:?service path is required}"
SHARED_PATH="${3:?shared path is required}"
INITIAL_VERSION="${4:-0.1.0}"
REPUBLISH_EXISTING="${5:-false}"
SERVICE_SCOPE="${6:-$SERVICE}"

write_semver_outputs() {
  local version="$1"
  IFS='.' read -r major minor patch <<< "$version"

  echo "major=$major" >> "$GITHUB_OUTPUT"
  echo "minor=$minor" >> "$GITHUB_OUTPUT"
  echo "patch=$patch" >> "$GITHUB_OUTPUT"
}

increment_semver() {
  local current="$1"
  local bump="$2"

  IFS='.' read -r major minor patch <<< "$current"

  case "$bump" in
    major)
      major=$((major + 1))
      minor=0
      patch=0
      ;;
    minor)
      minor=$((minor + 1))
      patch=0
      ;;
    patch)
      patch=$((patch + 1))
      ;;
    *)
      ;;
  esac

  echo "${major}.${minor}.${patch}"
}

detect_bump() {
  local commits="$1"
  local scoped_pattern="(${SERVICE_SCOPE}|shared|buildingblocks)"
  local major_found=false
  local minor_found=false
  local block subject scope body applies

  while IFS= read -r block; do
    [[ -z "$block" ]] && continue

    subject="$(printf '%s\n' "$block" | sed -n '1p')"
    body="$(printf '%s\n' "$block" | sed '1d')"
    applies=false
    scope=""

    if [[ "$subject" =~ ^[a-z]+$ ]]; then
      continue
    fi

    if [[ "$subject" =~ ^[a-z]+\(([^)]+)\)!: ]]; then
      scope="${BASH_REMATCH[1]}"
      [[ "$scope" =~ ^${scoped_pattern}$ ]] && applies=true
    elif [[ "$subject" =~ ^[a-z]+\(([^)]+)\): ]]; then
      scope="${BASH_REMATCH[1]}"
      [[ "$scope" =~ ^${scoped_pattern}$ ]] && applies=true
    elif [[ "$subject" =~ ^[a-z]+!: ]] || [[ "$subject" =~ ^[a-z]+: ]]; then
      applies=true
    fi

    [[ "$applies" != "true" ]] && continue

    if [[ "$subject" =~ !: ]] || grep -Eq 'BREAKING CHANGE:' <<< "$body"; then
      major_found=true
      break
    fi

    if [[ "$subject" =~ ^feat(\(|:) ]]; then
      minor_found=true
    fi
  done < <(printf '%s' "$commits" | awk -v RS='----DELIMITER----\n' 'NF')

  if [[ "$major_found" == "true" ]]; then
    echo "major"
  elif [[ "$minor_found" == "true" ]]; then
    echo "minor"
  else
    echo "patch"
  fi
}

git fetch --tags --force >/dev/null 2>&1 || true

LAST_TAG="$(git tag -l "${SERVICE}-v*" --sort=-version:refname | head -n 1 || true)"

if [[ -z "$LAST_TAG" ]]; then
  COMMITS="$(git log --format='%s%n%b----DELIMITER----' -- "$SERVICE_PATH" "$SHARED_PATH" || true)"

  if [[ -z "$COMMITS" ]]; then
    echo "should_release=false" >> "$GITHUB_OUTPUT"
    echo "reuse_existing_tag=false" >> "$GITHUB_OUTPUT"
    echo "last_tag=" >> "$GITHUB_OUTPUT"
    echo "current_version=0.0.0" >> "$GITHUB_OUTPUT"
    echo "next_version=0.0.0" >> "$GITHUB_OUTPUT"
    echo "tag_name=" >> "$GITHUB_OUTPUT"
    echo "bump=none" >> "$GITHUB_OUTPUT"
    write_semver_outputs "0.0.0"
    exit 0
  fi

  echo "should_release=true" >> "$GITHUB_OUTPUT"
  echo "reuse_existing_tag=false" >> "$GITHUB_OUTPUT"
  echo "last_tag=" >> "$GITHUB_OUTPUT"
  echo "current_version=0.0.0" >> "$GITHUB_OUTPUT"
  echo "next_version=$INITIAL_VERSION" >> "$GITHUB_OUTPUT"
  echo "tag_name=${SERVICE}-v${INITIAL_VERSION}" >> "$GITHUB_OUTPUT"
  echo "bump=initial" >> "$GITHUB_OUTPUT"
  write_semver_outputs "$INITIAL_VERSION"
  exit 0
fi

CURRENT_VERSION="${LAST_TAG#${SERVICE}-v}"
COMMITS="$(git log --format='%s%n%b----DELIMITER----' "${LAST_TAG}..HEAD" -- "$SERVICE_PATH" "$SHARED_PATH" || true)"

if [[ -z "$COMMITS" ]]; then
  if [[ "$REPUBLISH_EXISTING" == "true" ]]; then
    echo "should_release=true" >> "$GITHUB_OUTPUT"
    echo "reuse_existing_tag=true" >> "$GITHUB_OUTPUT"
  else
    echo "should_release=false" >> "$GITHUB_OUTPUT"
    echo "reuse_existing_tag=false" >> "$GITHUB_OUTPUT"
  fi

  echo "last_tag=$LAST_TAG" >> "$GITHUB_OUTPUT"
  echo "current_version=$CURRENT_VERSION" >> "$GITHUB_OUTPUT"
  echo "next_version=$CURRENT_VERSION" >> "$GITHUB_OUTPUT"
  echo "tag_name=$LAST_TAG" >> "$GITHUB_OUTPUT"
  echo "bump=none" >> "$GITHUB_OUTPUT"
  write_semver_outputs "$CURRENT_VERSION"
  exit 0
fi

BUMP="$(detect_bump "$COMMITS")"
NEXT_VERSION="$(increment_semver "$CURRENT_VERSION" "$BUMP")"

echo "should_release=true" >> "$GITHUB_OUTPUT"
echo "reuse_existing_tag=false" >> "$GITHUB_OUTPUT"
echo "last_tag=$LAST_TAG" >> "$GITHUB_OUTPUT"
echo "current_version=$CURRENT_VERSION" >> "$GITHUB_OUTPUT"
echo "next_version=$NEXT_VERSION" >> "$GITHUB_OUTPUT"
echo "tag_name=${SERVICE}-v${NEXT_VERSION}" >> "$GITHUB_OUTPUT"
echo "bump=$BUMP" >> "$GITHUB_OUTPUT"
write_semver_outputs "$NEXT_VERSION"
