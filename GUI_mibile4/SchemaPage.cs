using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Drawing;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using RuleModule_Odyn;
using System.Threading.Tasks;

namespace GUI_mobile4
{
    class SchemaPage : ContentPage, IReset
    {
        int x = 0, y = 0;
        int sX = -1, sY = -1;
        double scale = 1.0;
        readonly RuleModule rules;
        GaphicsSchemaAndroid gs;
        SKCanvas canvas;
        public SKCanvasView canvasView;
        Node remNode = null;
        Schema sc = null;
        public SchemaPage(RuleModule r)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            rules = r;
            PrepareSchema();
            canvasView = new SKCanvasView();
            canvasView.PaintSurface += OnCanvasViewPaintSurface;

            var pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += OnPinchUpdated;
            canvasView.GestureRecognizers.Add(pinchGesture);

            var panGesture = new PanGestureRecognizer();

            panGesture.PanUpdated += OnPanUpdated;
            canvasView.GestureRecognizers.Add(panGesture);

            Content = canvasView;
            Content.WidthRequest = 2000;
            Content.HeightRequest = 2000;

            Device.BeginInvokeOnMainThread(() => { ActiveNodePosition(); });
        }
        public void Reset()
        {
            //rules.ResetPath();
            remNode = null;
            RefreshPosition();
            canvasView.InvalidateSurface();
        }

            
        public void RefreshPosition()
        {
            Node nn = rules.GetCurrentNode();
            
            if (nn != null)
            {
                gs.NodeToScreen(nn.NodeName, (int)App.ScreenWidth, (int)App.ScreenHeight);
                canvasView.InvalidateSurface();
            }
            
        }
        async void ActiveNodePosition()
        {
            for (; ; )
            {
                if (rules.GetCurrentNode()!= remNode)
                {
                    remNode = rules.GetCurrentNode();
                    RefreshPosition();
                }
                await Task.Delay(1000);
            }
        }
        public SchemaPage(int x, RuleModule r)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            rules = r;
            canvasView = new SKCanvasView();
            canvasView.PaintSurface += OnCanvasViewPaintSurface;
            canvasView.HeightRequest = 1000;
            canvasView.WidthRequest = 1000;
            Content = canvasView;
            //bitmap = new SKBitmap(2000, 2000);
            //canvasBit = new SKCanvas(bitmap);
            var panGesture = new PanGestureRecognizer();

            //panGesture.PanUpdated += OnPanUpdated;
            //canvasView.GestureRecognizers.Add(panGesture);
            var pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += OnPinchUpdated;
            canvasView.GestureRecognizers.Add(pinchGesture);
        }
        void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            x = y = 0;
            if (e.Status == GestureStatus.Started)
            {
                var p = e.ScaleOrigin; 
               
                sX =(int) (p.X*canvasView.Width);
                sY =(int)(p.Y*canvasView.Height);
                // Store the current scale factor applied to the wrapped user interface element,
                // and zero the components for the center point of the translate transform.                
            }
            if (e.Status == GestureStatus.Running)
            {
               
                // Calculate the scale factor to be applied.
                //if(e.Scale>1)
                    scale =1+ (e.Scale - 1) * scale / 3;
                //else
                  //  scale =1- (e.Scale - 1) * scale/10;
                canvasView.InvalidateSurface();
            }
        }
            void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            scale = 1;
            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    x =(int)( e.TotalX/10);
                    y =(int)( e.TotalY/10);
                    //x= (int)Math.Max(Math.Min(0, x + e.TotalX), -Math.Abs(Content.Width - App.ScreenWidth))/10;
                    //y= (int)Math.Max(Math.Min(0, y + e.TotalY), -Math.Abs(Content.Height - App.ScreenHeight))/10;
                    //                    Content.TranslationX = Math.Max(Math.Min(0, x + e.TotalX), -Math.Abs(Content.Width - App.ScreenWidth));
                    //                    Content.TranslationY =Math.Max(Math.Min(0, y + e.TotalY), -Math.Abs(Content.Height - App.ScreenHeight));
                    canvasView.InvalidateSurface();
                    break;

                case GestureStatus.Completed:
                    // Store the translation applied during the pan
                    //x = (int)Content.TranslationX;
                    //y = (int)Content.TranslationY;
                    //canvasView.InvalidateSurface();
                    break;
            }
        }
        void PrepareSchema()
        {
            sc = rules.GetActiveSchema();
            gs = new GaphicsSchemaAndroid(sc.schema, sc.startNode);
            
            gs.PrepareSchemaDraw();
           
            gs.NodeToScreen(gs.startNode, (int)App.ScreenWidth, (int)App.ScreenHeight);
        }
        void DrawPath()
        {
            gs.ClearPath();
            if (sc.path.Count == 0)                            
                return;
                                    
            ColorOdyn c = new ColorOdyn(255, 0, 0);
            int start = 0;
            lock (sc.path)
            {
                if (sc.path.Count > 8)
                    start = sc.path.Count - 8;
                for (int i = start; i < sc.path.Count - 1; i++)
                {
                    gs.gn[sc.path[i]].SetConnectionColor(gs.gn[sc.path[i + 1]], c);
                    gs.gn[sc.path[i]].InPath();
                }

                gs.gn[sc.path[sc.path.Count - 1]].InPath();
            }
        }
        void ShowSchema(SKCanvas canvas)
        {
            gs.Scale(scale,sX,sY);          
            DrawPath();
/*            x = x - (int)((x - 0) * scale);
            x = y - (int)((y - 0) * scale);*/
            gs.DrawSchema(canvas, x, y);           
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            //  x = 0;
            // y = 0;
            
            SKSurface surface = args.Surface;
            canvas = surface.Canvas;
            canvas.Clear();
          

            ShowSchema(canvas);
        }
    }
}
