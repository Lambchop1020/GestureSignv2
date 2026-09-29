# Localization review — 2026-09-29

## Scope and evidence

All 90 shared Intent catalogs (231 strings each, 20,790 entries) were checked for key coverage, empty values, encoding damage, placeholder preservation, actual lookup and formatted sample/notification values. A separate full-catalog scan found no unchanged English sentences longer than 12 characters in non-English catalogs and no severe translation truncation. Fragment spacing differences were limited to Chinese punctuation and were retained intentionally.

This is a structural audit of every entry plus targeted linguistic review, **not a claim that every sentence in every language has been linguistically verified**. The rest of the legacy application's localization is outside this catalog audit.

## Corrections

656 entries in 89 locales were changed. The adjacent `proofreading-changes.json` records exact before/after text.

- Mouse button names specify the physical left/middle/right button; several languages previously translated Right as correct, Left as departed, or right as a legal entitlement.
- Model training labels no longer use railway/train or vocational retraining terminology in the identified cases.
- Clear label(s) means removing a label, rather than a clear/legible label.
- Corrected inverted or misleading unlabeled/on states, Serbian Cyrillic switch labels, and a repeated unrelated heading in Konkani.
- Corrected selected gesture names to attach the count to fingers and restored missing downward direction.
- Added regression checks for these semantic failures, opposite control labels and translation-service headings.

## Remaining linguistic review

All locales still require native-speaker acceptance before describing them as fully proofread. In particular Nynorsk remains partly based on Bokmål; Quechua, Konkani and other lower-resource languages require dialect, terminology and full-sentence review. No new automatic translation pass is presented as native review. Layout, right-to-left rendering and screen-reader pronunciation have not been exhaustively checked in all locales.
