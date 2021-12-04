using SpotifyHelper.Core.Extensions;
using System;
using Xunit;

namespace SpotifyHelper.Core.Tests;

public class EnumExtensions
{
    [Flags]
    internal enum TestEnum
    {
        None,
        First,
        Second
    }


    [Theory]
    [InlineData(true, TestEnum.First)]
    [InlineData(false, TestEnum.First)]
    [InlineData(true, TestEnum.Second)]
    internal void SetFlagIf_SetFlagIfConditionIsTrue(bool condition, TestEnum flag)
    {
        // Arrange
        var target = TestEnum.None;

        // Act
        target = target.SetFlagIf(flag, condition);

        // Asset
        var hasFlag = target.HasFlag(flag);

        Assert.Equal(hasFlag, condition);
    }

    [Fact]
    internal void SetFlagIf_SetFlagAlreadySet_NothingHappens()
    {
        // Arrange
        var target = TestEnum.None | TestEnum.First;

        // Act
        target = target.SetFlagIf(TestEnum.First, condition: true);

        // Assert
        var hasFirst = target.HasFlag(TestEnum.First);
        var hasSecond = target.HasFlag(TestEnum.Second);

        Assert.True(hasFirst);
        Assert.False(hasSecond);
    }
}
