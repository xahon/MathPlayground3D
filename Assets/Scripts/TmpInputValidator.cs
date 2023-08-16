using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "TmpInputValidator", menuName = "Input Validator")]
public class TmpInputValidator : TMP_InputValidator
{
    public override char Validate(ref string text, ref int pos, char ch)
    {
        if (ch == '`')
        {
            return '\0';
        }

        if (ch == '\t')
        {
            text = text.Insert(pos, "    ");
            pos += 4;
        }
        else
        {
            text = text.Insert(pos, ch.ToString());
            pos++;
        }
        return ch;
    }
}