namespace SharpAttributeParser;

using Moq;

using System.Collections.Generic;

internal static class StringComparerMock
{
    public static IEqualityComparer<string> CreateComparer(bool returnValue) => CreateMock(returnValue).Object;

    public static Mock<IEqualityComparer<string>> CreateMock(bool returnValue)
    {
        Mock<IEqualityComparer<string>> mock = new();

        mock.Setup(static (comparer) => comparer.Equals(It.IsAny<string>(), It.IsAny<string>())).Returns(returnValue);
        mock.Setup(static (comparer) => comparer.GetHashCode()).Returns(0);

        return mock;
    }

    public static void VerifyInvoked(Mock<IEqualityComparer<string>> mock) => mock.Verify(static (comparer) => comparer.Equals(It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);
}
