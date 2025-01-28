using UnityEngine;

[CreateAssetMenu(menuName = "Paddle Powers/Clone Power")]
public class PaddlePowerClone : PaddlePowerBase
{
    public float cloneDuration = 5f;
    public Color cloneColor = Color.green;
    public float tweenDuration = 0.5f;
    public int cloneCount = 1;

    private GameObject[] clones;

    public override void Activate(GameObject paddle)
    {
        PaddleController paddleController = paddle.GetComponent<PaddleController>();
        if (paddleController == null || paddleController.core == null)
        {
            Debug.LogError("No se encontró el núcleo o el PaddleController.");
            return;
        }

        Transform core = paddleController.core;
        float radius = Vector3.Distance(core.position, paddle.transform.position);
        float initialAngle = GetAngleFromCore(core.position, paddle.transform.position);

        clones = new GameObject[cloneCount];
        float angleStep = 360f / (cloneCount + 1);

        for (int i = 1; i <= cloneCount; i++)
        {
            float cloneAngle = initialAngle + (angleStep * i) % 360;
            Vector3 clonePosition = GetPositionOnOrbit(core.position, cloneAngle, radius);
            GameObject clone = Instantiate(paddle, clonePosition, paddle.transform.rotation);

            SetCloneVisuals(clone, paddle.transform.localScale);
            Vector3 originalScale = paddle.transform.localScale;
            clone.transform.localScale = Vector3.zero;
            LeanTween.scale(clone, originalScale, tweenDuration).setEaseOutBounce();

            PaddleCloneController cloneController = clone.AddComponent<PaddleCloneController>();
            cloneController.originalPaddle = paddle;
            cloneController.core = core;
            cloneController.MarkAsClone();

            clones[i - 1] = clone;
        }

        paddle.GetComponent<MonoBehaviour>().StartCoroutine(DestroyClonesAfterDuration());
    }

    private System.Collections.IEnumerator DestroyClonesAfterDuration()
    {
        yield return new WaitForSeconds(cloneDuration);

        foreach (GameObject clone in clones)
        {
            if (clone != null)
            {
                LeanTween.scale(clone, Vector3.zero, tweenDuration).setEaseInBack()
                    .setOnComplete(() => Destroy(clone));
            }
        }
    }

    private float GetAngleFromCore(Vector3 corePosition, Vector3 paddlePosition)
    {
        Vector3 direction = paddlePosition - corePosition;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    private Vector3 GetPositionOnOrbit(Vector3 corePosition, float angle, float radius)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(
            corePosition.x + Mathf.Cos(radian) * radius,
            corePosition.y + Mathf.Sin(radian) * radius,
            corePosition.z
        );
    }

    private void SetCloneVisuals(GameObject clone, Vector3 originalScale)
    {
        SpriteRenderer spriteRenderer = clone.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = cloneColor;
            spriteRenderer.flipX = true;
        }

        PaddlePower paddlePower = clone.GetComponent<PaddlePower>();
        if (paddlePower != null) paddlePower.enabled = false;
        clone.transform.localScale = originalScale;
    }
}