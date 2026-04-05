#!/usr/bin/env bash
set -euo pipefail

SERVICE="${1:?service is required}"
TAG_NAME="${2:?tag name is required}"
VERSION="${3:?version is required}"
REUSE_EXISTING_TAG="${4:-false}"

git config user.name "github-actions[bot]"
git config user.email "41898282+github-actions[bot]@users.noreply.github.com"

TAG_EXISTS=false
TAG_CREATED=false
RELEASE_EXISTS=false
RELEASE_CREATED=false

if git ls-remote --exit-code --tags origin "refs/tags/${TAG_NAME}" >/dev/null 2>&1; then
  TAG_EXISTS=true
fi

if [[ "$TAG_EXISTS" != "true" && "$REUSE_EXISTING_TAG" != "true" ]]; then
  git tag -a "$TAG_NAME" -m "${SERVICE} v${VERSION}"
  git push origin "$TAG_NAME"
  TAG_CREATED=true
  TAG_EXISTS=true
fi

if gh release view "$TAG_NAME" >/dev/null 2>&1; then
  RELEASE_EXISTS=true
else
  gh release create "$TAG_NAME" \
    --title "${SERVICE} v${VERSION}" \
    --generate-notes
  RELEASE_CREATED=true
fi

echo "tag_exists=$TAG_EXISTS" >> "$GITHUB_OUTPUT"
echo "tag_created=$TAG_CREATED" >> "$GITHUB_OUTPUT"
echo "release_exists=$RELEASE_EXISTS" >> "$GITHUB_OUTPUT"
echo "release_created=$RELEASE_CREATED" >> "$GITHUB_OUTPUT"
echo "tag_name=$TAG_NAME" >> "$GITHUB_OUTPUT"
