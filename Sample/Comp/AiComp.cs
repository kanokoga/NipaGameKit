using UnityEngine;

namespace NipaGameKit
{
    public class AiComp : CompMonoBase
    {
        public override int InitOrder { get; } = 100;

        protected CoreComp coreComp;


        public override void Init(int monoId)
        {
            base.Init(monoId);
           // this.coreComp = CompGroup<CoreComp>.GetComponent(this.MonoId);
        }

        public override void Dispose()
        {
            this.coreComp = null;
        }
    }
}
