using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<EventAggregator>().AsSingle().NonLazy();
        Container.Bind<ITimeService>().To<TimeManager>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
}