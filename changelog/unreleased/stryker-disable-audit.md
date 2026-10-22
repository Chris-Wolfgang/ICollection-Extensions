type: internal

Re-audited every Stryker disable in `ICollectionExtensions`: three null guards whose mutants were wrongly marked equivalent are now under test, and the two remaining guard disables no longer hide their condition mutants.
