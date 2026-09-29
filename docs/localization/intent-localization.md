# AI, accessibility and mouse settings localization

The recent AI learning/review, correction dialogs, accessibility controls, mouse start buttons, default gesture display names and AI veto notifications use a shared catalog in `GestureSign.Foundation/Localization/Intent`.

- 90 locale identifiers match `UiTranslationCatalog.SupportedCultureNames`.
- English keys are stable format templates. Insert counts, gesture names and other runtime values **after** translation.
- Catalogs are embedded in Foundation so the settings app and gesture daemon use the same strings. No translation service is used at runtime.
- Existing optional learning components may return Chinese status messages. The settings display boundary maps known messages to the selected locale. Stored sample data, gesture IDs, user-defined names and diagnostic logs are not rewritten.
- Existing inline/JSON localization for other application pages remains in place.

## Translation quality

The new translations started with Google machine translation, followed by placeholder, encoding and coverage validation and terminology corrections. They have **not** all been reviewed by native speakers. Regional variants share translations where appropriate; Serbian Latin uses transliteration from Serbian Cyrillic. Norwegian Nynorsk received vocabulary adjustments from the Norwegian draft and particularly needs native review. Quechua and other lower-resource language translations also need native review before describing linguistic quality as verified.

When adding text, update every locale and run `tests/GestureSign.LocalizationTests`. The test checks the embedded locale count, key parity, format placeholders, actual catalog lookup, dynamic sample/notification numbers, legacy runtime messages and source key coverage. It does not certify linguistic quality or inspect every translated layout.
