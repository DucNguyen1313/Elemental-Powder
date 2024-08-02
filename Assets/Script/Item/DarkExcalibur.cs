using UnityEngine;

public class DarkExcalibur : MonoBehaviour
{
    public BombController bombController;
    [SerializeField] protected GameObject finalSparkPrefab;

    protected FinalSpark finalSpark;
    public KeyCode inputKey = KeyCode.Space;

    private void Awake()
    {
        inputKey = bombController.inputKey;
    }

    void Update()
    {
        if (Input.GetKeyDown(inputKey) && PlayerStatus.Instance.Weapons[Item.DarkExcalibur] > 0)
        {
            //Debug.Log(PlayerStatus.Instance.Weapons[Item.DarkExcalibur]);
            Debug.Log("Attack by DarkExca");
            DarkExcaliburAttack();
            PlayerStatus.Instance.RemoveWeaponQuantity(Item.DarkExcalibur, 1);
        }
    }

    void DarkExcaliburAttack()
    {
        GameObject darkExca = Instantiate(finalSparkPrefab, this.transform.position, this.transform.rotation);
        //darkExca.transform.parent = this.transform;

        Vector2 direction = PlayerMovement.Instance.Direction;
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            darkExca.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            darkExca.transform.position += new Vector3(direction.x, direction.y, 0f);
        }
    }
}
