using Microsoft.AspNetCore.Components.Forms;

namespace StudentPortal.WEB.Shared.Forms
{
    /// <summary>
    /// Campo de entrada para GUIDs; se comporta igual que InputText pero con tipo Guid.
    /// </summary>
    public class InputGuid : InputBase<Guid>
    {
        protected override bool TryParseValueFromString(
            string? value,
            out Guid result,
            out string? validationErrorMessage)
        {
            if (Guid.TryParse(value, out var guid))
            {
                result = guid;
                validationErrorMessage = null;
                return true;
            }

            validationErrorMessage = "El valor no es un GUID válido.";
            result = default;
            return false;
        }

        protected override string FormatValueAsString(Guid value) =>
            value == Guid.Empty ? string.Empty : value.ToString();
    }
}
