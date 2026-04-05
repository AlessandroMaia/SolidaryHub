#!/usr/bin/env bash
set -euo pipefail

SERVICE="${1:?service is required}"
SHARED_CHANGED="${2:-false}"
SERVICE_CHANGED="${3:-false}"
MANUAL_TARGET="${4:-auto}"

IMPACTED=false
REASON=none

case "$MANUAL_TARGET" in
  auto)
    if [[ "$SHARED_CHANGED" == "true" || "$SERVICE_CHANGED" == "true" ]]; then
      IMPACTED=true

      if [[ "$SHARED_CHANGED" == "true" && "$SERVICE_CHANGED" == "true" ]]; then
        REASON="shared-and-service"
      elif [[ "$SHARED_CHANGED" == "true" ]]; then
        REASON="shared"
      else
        REASON="service"
      fi
    fi
    ;;
  all)
    IMPACTED=true
    REASON="manual-all"
    ;;
  "$SERVICE")
    IMPACTED=true
    REASON="manual-service"
    ;;
  *)
    IMPACTED=false
    REASON="manual-skip"
    ;;
esac

echo "service=$SERVICE" >> "$GITHUB_OUTPUT"
echo "impacted=$IMPACTED" >> "$GITHUB_OUTPUT"
echo "reason=$REASON" >> "$GITHUB_OUTPUT"
