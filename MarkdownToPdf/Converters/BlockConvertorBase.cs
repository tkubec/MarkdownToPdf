using Markdig.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownToPdf
{
    public abstract class BlockConvertorBase<TBlock> : IBlockConverter
        where  TBlock: Block
        
    {
        public  TBlock CurrentBlock { get; }

        public Block Block { get => CurrentBlock; }
        public ContainerBlockConverter Parent { get; }

        public MigraDocBlockContainer Output { get;  set; }
        public MarkdownToPdf Owner { get;  protected set; }
        public ElementAttributes Attributes { get; protected set; }
        public MarkdownStyle Style{ get; protected set; }
        public InheritedData Inherited { get; protected set; }
        public string RawText { get; protected set; }
        public List<string> Lines { get; protected set; }

        public ElementDescriptor Descriptor { get; protected set; }

        public BlockConvertorBase(TBlock node,  ContainerBlockConverter parent)
        {
            CurrentBlock = node;
            Parent = parent;
            Owner = parent?.Owner ?? Owner;
            RawText= parent?.RawText?? RawText;
            Lines = parent?.Lines ?? Lines;

            Descriptor = new ElementDescriptor();
            Output = parent?.Output ?? Output;
            Inherited =  new InheritedData();

            if(Parent != null)
            {
                Inherited = new InheritedData(parent.Inherited);
                Inherited.NodeCount = Parent.CurrentBlock.Count();
                Inherited.NodeIndex = Parent.CurrentBlock.IndexOf(CurrentBlock);
            }
        }


        protected MarkdownStyle GetStyle()
        {
            var d = GetDescriptors();
            var style = Owner?.StyleManager.GetStyle(d);
            return style;
        }

        public virtual void Convert()
        {
            CreateOutput();
            ApplyStyling();
            ConvertContent();
        }


        public List<ElementDescriptor> GetDescriptors(List<ElementDescriptor> descriptors = null)
        {
            descriptors = descriptors == null ? new List<ElementDescriptor>() : descriptors;

            descriptors.Add(Descriptor);

            if (Parent != null && Parent.GetType() != typeof(RootBlockConvertor))
            {
                Parent.GetDescriptors(descriptors);
            }
            return descriptors;
        }

        public  List<IBlockConverter> GetParentsEndedByThis()
        {
            var res = new List<IBlockConverter>();
            var i = this as IBlockConverter;

            while (i.Inherited.NodeCount - 1 == i.Inherited.NodeIndex)
            {
                res.Add(i);
                if (i.Parent != null) i = i.Parent;
                else break;
            }

            return res;
        }

        protected abstract void ConvertContent();
        protected abstract void ApplyStyling();
        protected abstract void CreateOutput();
    }
}
