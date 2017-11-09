using System;
using System.Windows.Media;

namespace WPF.Extend
{
    public interface IRenderSource : IDisposable
    {
        bool CheckFormat(FrameFormat format);

        bool SetupSurface(int videoWidth, int videoHeight, FrameFormat format);

        void Render(IntPtr buffer);

        void Render(IntPtr yBuffer, IntPtr uBuffer, IntPtr vBuffer);

        ImageSource ImageSource { get; }

        event EventHandler ImageSourceChanged;
    }
}
