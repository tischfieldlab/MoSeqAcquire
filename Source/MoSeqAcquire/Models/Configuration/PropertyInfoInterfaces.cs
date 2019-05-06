using System.Collections.Generic;

namespace MoSeqAcquire.Models.Configuration
{
    public interface IRangeInfo
    {
        object Min { get; }
        object Max { get; }
        object Step { get; }
    }
    public interface IDefaultInfo
    {
        object Default { get; }
    }
    public interface IAutomaticInfo
    {
        bool AllowsAuto { get; }
    }
    public interface ISupportInfo
    {
        bool IsSupported { get; }
    }
    public interface IChoicesProvider
    {
        IEnumerable<object> Choices { get; }
        string DisplayPath { get; }
        string ValuePath { get; }
    }
}
