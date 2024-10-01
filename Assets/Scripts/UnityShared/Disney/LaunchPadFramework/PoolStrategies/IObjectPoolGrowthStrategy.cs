namespace Disney.LaunchPadFramework.PoolStrategies
{
	public interface IObjectPoolGrowthStrategy
	{
		void Grow(GameObjectPool pool);

		void Grow<T>(ObjectPool<T> pool) where T : class, new();
	}
}
