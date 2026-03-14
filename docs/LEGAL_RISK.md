# LEGAL_RISK

Status: draft v0

## Purpose
Track the legal and platform risks of building a commercially targeted digital game that is intentionally faithful to Binho-style gameplay while preserving a fast path to an original/generic presentation.

## Main risk areas

### 1. Trademark / branding risk
Risks may include:
- product name usage
- logo usage
- store listing language
- branded team/theme references

## 2. Trade dress / look-and-feel risk
Risks may include:
- highly recognizable board presentation
- distinctive visual layout if copied too literally in commercial branding contexts
- packaging/store art that suggests official association

## 3. Copyright risk
Risks may include:
- copying official rule text verbatim into product copy
- reusing official art, videos, graphics, or layout diagrams
- reproducing branded assets without permission

## 4. Product/platform review risk
Even without litigation risk, App Store review may object to:
- confusing similarity
- misleading naming or metadata
- implication of affiliation

## Mitigation strategy

### Architecture mitigation
- keep gameplay engine separate from theme/content layer
- maintain a fully generic ship-capable theme pack at all times
- keep branded copy/assets out of core codepaths

### Content mitigation
- rewrite all user-facing rule/tutorial copy in original wording
- create original art/UI/branding even if gameplay is highly faithful
- avoid using official logos/names in fallback/default shipping assets

### Product mitigation
- treat exact-clone branding as internal/prototype-sensitive mode
- prepare a generic/original public identity in parallel
- document all places where branding-sensitive assets appear

## Operational policy
Before App Store submission, perform a full branding scrub and decide one of:
- pursue licensed/authorized branding path
- submit under generic/original identity

## Current recommendation
Proceed with mechanics research and engine design now, but assume public release may require the generic/original theme path unless explicit rights are secured.
