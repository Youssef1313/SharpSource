using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpSource.Diagnostics;
using SharpSource.Test.Helpers;

namespace SharpSource.Test;

[TestClass]
public class ThrowNullTests : DiagnosticVerifier
{
    protected override DiagnosticAnalyzer DiagnosticAnalyzer => new ThrowNullAnalyzer();

    [TestMethod]
    public async Task ThrowNull_ThrowsNullLiteral()
    {
        var original = @"throw null;";

        await VerifyDiagnostic(original, "Throwing null will always result in a runtime exception");
    }

    [TestMethod]
    public async Task ThrowNull_ThrowsNullConstant()
    {
        var original = @"
const System.Exception NullConst = null;
throw NullConst;";

        await VerifyDiagnostic(original, "Throwing null will always result in a runtime exception");
    }

    [TestMethod]
    public async Task ThrowNull_ThrowsNullCast()
    {
        var original = @"throw (System.Exception)null;";

        await VerifyDiagnostic(original, "Throwing null will always result in a runtime exception");
    }

    [TestMethod]
    public async Task ThrowNull_DoesNotThrowNull()
    {
        var original = @"throw new System.Exception();";

        await VerifyDiagnostic(original);
    }

    [TestMethod]
    public async Task ThrowNull_Rethrow()
    {
        var original = @"
try {

} catch (System.Exception) {
    throw;
}";

        await VerifyDiagnostic(original);
    }
}