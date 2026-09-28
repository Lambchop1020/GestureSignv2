using System;
using GestureSign.Daemon.Input;
class Program {
 static int checks;
 static void Check(bool ok,string label) { if(!ok)throw new Exception(label); checks++; }
 static void Main() {
  var g=new EdgeClickGate();
  Check(!g.Filter(true,false),"normal left down"); Check(!g.Filter(false,false),"normal up");
  g.Begin(false); Check(g.NeedsHook,"active retains hook");
  Check(g.Filter(true,false),"log replay 15:14:53.894 blocks left down");
  g.End(); Check(g.NeedsHook,"release retains hook for delayed up");
  Check(!g.Filter(true,true),"configured injected left down allowed");
  Check(!g.Filter(false,true),"injected up does not clear native pair");
  Check(g.Filter(false,false),"log replay 15:14:54.111 blocks late native up");
  Check(!g.NeedsHook,"pair completed releases hook");
  Check(!g.Filter(true,false),"next normal click unaffected");Check(!g.Filter(false,false),"next up unaffected");
  g.Begin(true);Check(!g.Filter(true,false),"preheld drag down preserved");Check(!g.Filter(false,false),"preheld drag up preserved");
  Check(g.Filter(true,false),"new click after preheld drag claimed");Check(g.Filter(false,false),"matching up claimed");g.End();
  g.Begin(false);Check(!g.Filter(false,false),"unmatched up passes");g.End();Check(!g.Filter(true,false),"no broad post-edge timeout");
  g.Begin(false);Check(g.Filter(true,false),"missing-up pair started");g.End();Check(!g.Filter(true,false),"fresh outside down recovers missing up");Check(!g.Filter(false,false),"recovered pair up passes");
  g.Begin(false);Check(g.Filter(true,false),"overlap down claimed");g.End();g.Begin(true);Check(g.Filter(false,false),"next edge does not forget pending up");g.End();
  Console.WriteLine($"PASS: {checks} edge click pair checks.");
 }
}
