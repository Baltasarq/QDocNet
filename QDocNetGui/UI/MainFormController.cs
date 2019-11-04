﻿namespace QDocNetGui.UI
{
    using System.Windows.Forms;
    
    using QDocNetLib;
    using QDocNetLib.Persistence;
    
    public class MainFormController
    {
        public MainFormController()
        {
            this.Form = new MainFormView();
            
            this.Form.Closed += (o, evt) => Application.Exit();
            this.Form.TbBar.ButtonClick += (o, evt) => this.OnToolbarClicked( evt );
            this.Form.TreeView.NodeMouseDoubleClick += (o, args) => this.OnNodeClicked();
        }
        
        void OnToolbarClicked(ToolBarButtonClickEventArgs args)
        {
            switch( this.Form.TbBar.Buttons.IndexOf( args.Button ) ) {
                case 0:
                    this.OnOpen();
                    break;
                case 1:
                    this.OnSave();
                    break;
                case 2:
                    this.OnAbout();
                    break;
                default:
                    MessageBox.Show( "Unsupported toolbar button clicked" );
                    break;
            }

            return;
        }
        
        void OnNodeClicked()
        {
            TreeNode node = this.Form.TreeView.SelectedNode;
        
            if ( node is DocTreeNode docNode ) {
                this.Form.TbDoc.Text = docNode.Entity.Docs.ToString();
            }
            
            return;
        }

        void OnOpen()
        {
            using (var openDlg = new OpenFileDialog() ) {
                openDlg.Filter = "XML doc files (*.xml)|*.xml|All files|*.*";
                
                if ( openDlg.ShowDialog() == DialogResult.OK ) {
                    this.Unit = Helper.LoadDocs( openDlg.FileName );
                    
                    this.LoadDocs();
                }
            }
        }

        void OnSave()
        {
            using( var saveDlg = new SaveFileDialog() ) {
                saveDlg.Filter = "html (*.html)|*.html";
                saveDlg.DefaultExt = "html";
                saveDlg.CheckPathExists = true;
                
                if ( saveDlg.ShowDialog() == DialogResult.OK ) {
                    new HtmlExporter( this.Unit ).SaveTo( saveDlg.FileName );
                }
            }
        }

        void OnAbout()
        {
            this.Form.PnlAbout.Show();
        }
        
        void LoadEntity(TreeNode node, Entity entity)
        {
            foreach(Entity subEntity in entity.Entries) {
                var subNode = new DocTreeNode( subEntity );
                
                node.Nodes.Add( subNode );
                
                this.LoadEntity( subNode, subEntity );
            }
            
            return;
        }
        
        void LoadDocs()
        {
            if ( this.Unit != null ) {
                TreeNode root = this.Form.TreeView.Nodes.Add( this.Unit.Name );
                
                foreach(Entity cls in this.Unit.Classes) {
                    var subNode = new DocTreeNode( cls );
                    
                    root.Nodes.Add( subNode );
                    this.LoadEntity( subNode, cls );
                }
                
                root.Expand();
            } else {
                MessageBox.Show( "No docs loaded." );
            }
            
            return;
        }
        
        public MainFormView Form {
            get; private set;
        }
        
        public Unit Unit {
            get; private set;
        }
    }
}
