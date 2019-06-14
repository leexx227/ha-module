------------------------------- MODULE hpcha -------------------------------
EXTENDS Integers, FiniteSets

CONSTANT SERVER

VARIABLE serverState
VARIABLE currentLeader
VARIABLE serverHeartBeat

vars == <<serverState, currentLeader, serverHeartBeat>>

TypeOK == 
  /\ serverState \in [SERVER -> {"follower", "leader"}]
  /\ serverHeartBeat \in [SERVER -> {"heartbeating", "stopped"}]
  /\ currentLeader \subseteq SERVER
  
Init ==
  /\ serverState = [s \in SERVER |-> "follower"]
  /\ serverHeartBeat = [s \in SERVER |-> "stopped"]
  /\ currentLeader = {}
  
StartHeartBeat(s) ==
  /\ \/ currentLeader = {}
     \/ serverState[s] = "leader"
  /\ serverHeartBeat' = [serverHeartBeat EXCEPT ![s] = "heartbeating"]
  /\ UNCHANGED <<currentLeader, serverState>>
  
StopHeartBeat(s) ==
  /\ currentLeader # {}
  /\ s \notin currentLeader 
  /\ serverHeartBeat' = [serverHeartBeat EXCEPT ![s] = "stopped"]
  /\ UNCHANGED <<currentLeader, serverState>>
  
ServerCrach(s) ==
  /\ serverState' = [serverState EXCEPT ![s] = "follower"]
  /\ serverHeartBeat' = [serverHeartBeat EXCEPT ![s] = "stopped"]
  /\ UNCHANGED <<currentLeader>>
  
ServerConnectionLost(s) ==
  /\ serverHeartBeat' = [serverHeartBeat EXCEPT ![s] = "stopped"]
  /\ UNCHANGED <<currentLeader, serverState>>
  
StepUp(s) ==
  /\ serverState[s] = "follower"
  /\ s \in currentLeader
  /\ serverState' = [serverState EXCEPT ![s] = "leader"]
  /\ UNCHANGED <<currentLeader, serverHeartBeat>>  
  
StepDown(s) ==
  /\ serverState[s] = "leader"
  /\ s \notin currentLeader
  /\ serverState' = [serverState EXCEPT ![s] = "follower"]
  /\ serverHeartBeat' = [serverHeartBeat EXCEPT ![s] = "stopped"]
  /\ UNCHANGED <<currentLeader>>

LeaderLost ==
  /\ currentLeader # {}
  /\ \E s \in currentLeader: 
     /\ serverHeartBeat[s] = "stopped"
     /\ currentLeader' = currentLeader \ {s}
  /\ UNCHANGED <<serverState, serverHeartBeat>>
  
LeaderElected ==
  /\ currentLeader = {}
  /\ \E s \in SERVER:
      /\ serverHeartBeat[s] = "heartbeating"
      /\ currentLeader' = currentLeader \cup {s}
  /\ UNCHANGED <<serverHeartBeat, serverState>>
  
Next == 
  \/ LeaderElected \/ LeaderLost
  \/ \E s \in SERVER: \/ ServerCrach(s)
                      \/ StartHeartBeat(s)
                      \/ StopHeartBeat(s)
                      \/ ServerConnectionLost(s)
                      \/ StepUp(s)
                      \/ StepDown(s)
                    
SingleLeaderOnly == Cardinality(currentLeader) <= 1   

Spec == Init /\ [][Next]_vars
FairSpec == /\ Spec 
            /\ SF_vars(LeaderElected) /\ WF_vars(LeaderLost) 
            /\ \A s \in SERVER: /\ SF_vars(StepUp(s)) 
                                /\ WF_vars(StepDown(s))
                                /\ WF_vars(StartHeartBeat(s))
                                /\ WF_vars(StopHeartBeat(s))
                        
EventualHeartBeat == currentLeader = {} ~> (\E s \in SERVER: serverHeartBeat[s] = "heartbeating")
EventualElected == (\E s \in SERVER: serverHeartBeat[s] = "heartbeating") ~> currentLeader # {}
EventualStepUp == currentLeader # {} ~> (\E s \in SERVER: serverState[s] = "leader")
                            
=============================================================================
\* Modification History
\* Last modified Fri Jun 14 20:34:08 CST 2019 by zihche
\* Created Wed Jun 12 18:37:17 CST 2019 by zihche
