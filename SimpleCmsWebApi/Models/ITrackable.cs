using System;

namespace SimpleCmsWebApi.Models
{
    public interface ITrackable
    {
        DateTime Timestamp { get; set; }
    }
}