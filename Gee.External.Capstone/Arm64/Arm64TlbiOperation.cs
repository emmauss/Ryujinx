using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     ARM64 Translation Lookaside Buffer (TLBI) Operation.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Arm64TlbiOperation {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, TLBI operation.
        /// </summary>
        Invalid = 0,
        ARM64_TLBI_VMALLE1IS,
        ARM64_TLBI_VAE1IS,
        ARM64_TLBI_ASIDE1IS,
        ARM64_TLBI_VAAE1IS,
        ARM64_TLBI_VALE1IS,
        ARM64_TLBI_VAALE1IS,
        ARM64_TLBI_ALLE2IS,
        ARM64_TLBI_VAE2IS,
        ARM64_TLBI_ALLE1IS,
        ARM64_TLBI_VALE2IS,
        ARM64_TLBI_VMALLS12E1IS,
        ARM64_TLBI_ALLE3IS,
        ARM64_TLBI_VAE3IS,
        ARM64_TLBI_VALE3IS,
        ARM64_TLBI_IPAS2E1IS,
        ARM64_TLBI_IPAS2LE1IS,
        ARM64_TLBI_IPAS2E1,
        ARM64_TLBI_IPAS2LE1,
        ARM64_TLBI_VMALLE1,
        ARM64_TLBI_VAE1,
        ARM64_TLBI_ASIDE1,
        ARM64_TLBI_VAAE1,
        ARM64_TLBI_VALE1,
        ARM64_TLBI_VAALE1,
        ARM64_TLBI_ALLE2,
        ARM64_TLBI_VAE2,
        ARM64_TLBI_ALLE1,
        ARM64_TLBI_VALE2,
        ARM64_TLBI_VMALLS12E1,
        ARM64_TLBI_ALLE3,
        ARM64_TLBI_VAE3,
        ARM64_TLBI_VALE3
    }
}