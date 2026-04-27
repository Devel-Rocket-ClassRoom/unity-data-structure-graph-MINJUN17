using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiGraphNode : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI text;
    private GraphNode node;
    public void Reset()
    {
        SetColor(node.CanVisit ? Color.white : Color.grey);
        SetText($"ID: {node.id}\nweight: {node.weight}");
    }
    public void SetNode(GraphNode node)
    {
        this.node = node;
    }
    public void SetColor(Color color)
    {
        image.color = color;
    }
    public void SetText(string text)
    {
        this.text.text = text;
    }
}
