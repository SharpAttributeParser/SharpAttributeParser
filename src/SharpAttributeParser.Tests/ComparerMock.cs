namespace SharpAttributeParser.Tests;

using Moq;

using System.Collections.Generic;

internal static class ComparerMock
{
    public static Mock<IEqualityComparer<string>> GetComparer(bool returnValue)
    {
        Mock<IEqualityComparer<string>> mock = new();

        mock.Setup(static (comparer) => comparer.Equals(It.IsAny<string>(), It.IsAny<string>())).Returns(returnValue);

        return mock;
    }

    public static void VerifyInvoked(Mock<IEqualityComparer<string>> mock) => mock.Verify(static (comparer) => comparer.Equals(It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);
}
