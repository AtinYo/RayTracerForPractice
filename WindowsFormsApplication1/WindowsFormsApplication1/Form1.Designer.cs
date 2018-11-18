using WindowsFormsApplication1.Ray_Tracing;

namespace WindowsFormsApplication1
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(-1, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(960, 540);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 501);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        /// <summary>
        /// 到时候就用下面这个来渲染图片
        /// </summary>
        public void RenderPicture()
        {
            Scene scene = new Scene();
            scene.SetLightSource(new BaseLight(new Vec3(-1, -1, 3)));
            scene.AddRenderObj(new Sphere(new Vec3(0, 0, -300), 100f));
            Camera camera = new Camera(new Vec3(0, 0, 0), new Vec3(0, 0, -1), new Vec3(0, 1, 0), 960, 540, 500);

            bitmap = new System.Drawing.Bitmap(960, 540);
            for (int i = 0; i < 960; i++)
            {
                for (int j = 0; j < 540; j++)
                {
                    bitmap.SetPixel(i, j, scene.GetColor(i + 1, j + 1, camera));
                }
            }
            pictureBox1.Image = bitmap;
        }

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Drawing.Bitmap bitmap;
    }
}

