using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Health")]
    [SerializeField] private float _health;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _res;

    [Header("Movement")]
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _speed;
    [SerializeField] private float _sprintMul;
    [SerializeField] private float _crouchMul;
    [SerializeField] private float _jumpHeight;

    [Header("Height")]
    [SerializeField] private float _standingHeight;
    [SerializeField] private float _crouchingHeight;

    [Header("Dmg")]
    [SerializeField] private float _dmg;
    [SerializeField] private float _currentXp;
    [SerializeField] private float _requiredXp;

    public UnityEvent statChange;
   

    [Header("Xp")]
    [SerializeField] private int _level;
    [SerializeField] private UnityEvent levelChange;

    private void OnEnable()
    {
        _health = _maxHealth;

        _level = 0;
        _currentXp = 0;
        calculateRequiredXp();

    }
    //health
    public float getHealth()
    {
        return _health;
    }
    public float getMaxHealth()
    {
        return _maxHealth;
    }
    public float getRes()
    {
        return _res;
    }
    public void takeDmg(float dmg)
    {
        _health -= dmg;
        statChange.Invoke();
    }
    public void restoreHealth(float heal)
    {
        _health += heal;
        _health = Mathf.Clamp(_health, 0, _maxHealth);

        statChange.Invoke();
    }

    //Movement
    public float getMaxSpeed()
    {
        return _maxSpeed;
    }
    public float getSpeed()
    {
        return _speed;
    } 
    public float getSprintMul()
    {
        return _sprintMul;
    }
    public float getCrouchMul()
    {
        return _crouchMul;
    }
    public float getJumpHeight()
    {
        return _jumpHeight;
    }
    public float getStandingHeight()
    {
        return _standingHeight;
    }
    public float getCrouchingHeight()
    {
        return _crouchingHeight;
    }
    //dmg
    public float getDmg()
    {
        return _dmg;
    }
    //xp
    public float getCurrentXp()
    {
        return _currentXp;
    }
    public float getRequiredXp()
    {
        return _requiredXp;
    }
    public void gainXp(float gain)
    {
        _currentXp += gain;
        if (_currentXp > _requiredXp)
            levelUp();
        statChange.Invoke();
    }
    public int getLevel()
    {
        return _level;
    }
    public void levelUp()
    {
        _level++;
        _currentXp = Mathf.RoundToInt(_currentXp - _requiredXp);
        increaseHealth(1);
        restoreHealth(5);
        increaseDmg(2);
        calculateRequiredXp();
        levelChange.Invoke();

    }
    public void increaseHealth(int increase)
    {
        _maxHealth += increase;
    }
    public void increaseDmg(int increase)
    {
        _dmg += increase;
    }
    public void increaseSpd(int increase)
    {
        _speed += increase;
    }
    private void calculateRequiredXp()
    {
        int newRequired = 0;
        //brotato requiredXp scaling 
        newRequired = (_level + 3) * (_level + 3);

        _requiredXp = newRequired;
    }
}
