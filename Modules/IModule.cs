using System;

namespace DivineVision.Modules;

public interface IModule : IDisposable
{
    bool Enabled { get; set; }
    void OnUpdate();
    void OnDraw(); // новый метод
}