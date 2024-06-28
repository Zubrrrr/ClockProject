using UnityEngine;
using Zenject;

public class TimeServiceInstaller : MonoInstaller
{
    [SerializeField] TimeManager _timeManager;

    public override void InstallBindings()
    {
        Container.Bind<ITimeService>().FromInstance(_timeManager).AsSingle();
    }
}