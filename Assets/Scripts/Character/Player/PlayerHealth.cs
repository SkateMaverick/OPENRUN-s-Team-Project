using UnityEngine;

public class PlayerHealth : LivingEntity
{
    public static event System.Action OnPlayerDied;

    [Header("Immunity")]
    [SerializeField] private bool isImmune = false;

    public bool IsImmune
    {
        get => isImmune;
        set => isImmune = value;
    }

    protected override void Awake()
    {
        base.Awake();

        if (gameObject.name.Contains("Que"))
        {
            isImmune = true;
        }
        else if (gameObject.name.Contains("Noa"))
        {
            isImmune = false;
        }
    }

    // 플레이어가 대미지를 받았을 때의 처리
    public override void TakeDamage(int damage)
    {
        if (isImmune)
        {
            return;
        }

        base.TakeDamage(damage);
        
        // 애니메이션, ui 등 변경사항
    }

    // 플레이어가 체력이 다했을 때의 처리
    protected override void Die()
    {
        base.Die();
        
        OnPlayerDied?.Invoke();

        if (GameOverUI.Instance != null)
        {
            GameOverUI.Instance.ShowGameOver();
        }
        else
        {
            var ui = Object.FindFirstObjectByType<GameOverUI>();
            if (ui != null)
            {
                ui.ShowGameOver();
            }
        }
    }
}
