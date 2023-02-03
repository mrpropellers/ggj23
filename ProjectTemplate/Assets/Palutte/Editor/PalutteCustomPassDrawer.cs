using UnityEditor;
using UnityEditor.Rendering.HighDefinition;
using UnityEngine;

[CustomPassDrawer(typeof(PalutteCustomPass))]
public class PalutteCustomPassDrawer : CustomPassDrawer
{
    int m_Lines;
    SerializedProperty m_LutTexture;
    SerializedProperty m_GridWidth;
    SerializedProperty m_GridHeight;
    SerializedProperty m_MatchCamSize;
    SerializedProperty m_PixelsWidth;
    SerializedProperty m_PixelsHeight;
    SerializedProperty m_DitherMatrix;
    SerializedProperty m_DitherAmount;
    SerializedProperty m_ColorSpaceCompressionFactor;
    SerializedProperty m_JaggiesAreGood;

    protected override PassUIFlag commonPassUIFlags => PassUIFlag.Name;

    protected override void Initialize(SerializedProperty customPass)
    {
        m_LutTexture = customPass.FindPropertyRelative(nameof(PalutteCustomPass.LUTTexture));

        m_GridWidth = customPass.FindPropertyRelative(nameof(PalutteCustomPass.gridWidth));
        m_GridHeight = customPass.FindPropertyRelative(nameof(PalutteCustomPass.gridHeight));

        m_MatchCamSize = customPass.FindPropertyRelative(nameof(PalutteCustomPass.matchCamSize));

        m_PixelsWidth = customPass.FindPropertyRelative(nameof(PalutteCustomPass.pixelsWidth));
        m_PixelsHeight = customPass.FindPropertyRelative(nameof(PalutteCustomPass.pixelsHeight));

        m_DitherMatrix = customPass.FindPropertyRelative(nameof(PalutteCustomPass.ditherMatrix));
        m_DitherAmount = customPass.FindPropertyRelative(nameof(PalutteCustomPass.ditherAmount));

        m_ColorSpaceCompressionFactor = customPass.FindPropertyRelative(nameof(PalutteCustomPass.colorSpaceCompressionPower));

        m_JaggiesAreGood = customPass.FindPropertyRelative(nameof(PalutteCustomPass.jaggiesAreGood));
    }

    protected override void DoPassGUI(SerializedProperty customPass, Rect rect)
    {
        m_Lines = 0;
        rect.height = EditorGUIUtility.singleLineHeight;
        AddPropertyField(m_LutTexture, ref rect);
        AddPropertyField(m_GridWidth, ref rect);
        AddPropertyField(m_GridHeight, ref rect);
        AddDividingLine(ref rect);

        AddPropertyField(m_MatchCamSize, ref rect);
        if (!m_MatchCamSize.boolValue)
        {
            AddPropertyField(m_PixelsWidth, ref rect);
            AddPropertyField(m_PixelsHeight, ref rect);
        }
        AddDividingLine(ref rect);

        AddPropertyField(m_DitherMatrix, ref rect);
        AddPropertyField(m_DitherAmount, ref rect);
        AddDividingLine(ref rect);

        AddPropertyField(m_ColorSpaceCompressionFactor, ref rect);
        AddDividingLine(ref rect);

        AddPropertyField(m_JaggiesAreGood, ref rect);
    }

    protected override float GetPassHeight(SerializedProperty customPass)
    {
        return m_Lines * EditorGUIUtility.singleLineHeight;
    }

    void AddPropertyField(SerializedProperty property, ref Rect rect)
    {
        EditorGUI.PropertyField(rect, property);
        rect.y += EditorGUIUtility.singleLineHeight;
        m_Lines++;
    }

    void AddDividingLine(ref Rect rect)
    {
        rect.y += EditorGUIUtility.singleLineHeight;
        m_Lines++;
    }
}
