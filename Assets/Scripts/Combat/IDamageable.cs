/// <summary>Anything a shell can hurt.</summary>
public interface IDamageable
{
    Team Team { get; }

    void TakeDamage(int amount);
}
