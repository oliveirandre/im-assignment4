package scxmlgen.Modalities;

import scxmlgen.interfaces.IOutput;



public enum Output implements IOutput{
    
    NEWS("[from][FUSION][action][NEWS]"), //ta
    LEAVE("[from][FUSION][action][LEAVE]"),
    JOIN("[from][FUSION][action][JOIN]"),
    UMBRELLA_AVEIRO("[from][FUSION][action][UMBRELLA][subject][AVEIRO]"),
    UMBRELLA_PORTO("[from][FUSION][action][UMBRELLA][subject][PORTO]"),
    MUTE_JOAO("[from][FUSION][action][MUTE][subject][JOAO]"),
    MUTE_ANDRE("[from][FUSION][action][MUTE][subject][ANDRE]"),
    UNMUTE_JOAO("[from][FUSION][action][UNMUTE][subject][JOAO]"),
    UNMUTE_ANDRE("[from][FUSION][action][UNMUTE][subject][ANDRE]"),
    MUTE_TODOS("[from][FUSION][action][MUTE][subject][TODOS]"),
    UNMUTE_TODOS("[from][FUSION][action][UNMUTE][subject][TODOS]");
    
    private String event;

    Output(String m) {
        event=m;
    }
    
    public String getEvent(){
        return this.toString();
    }

    public String getEventName(){
        return event;
    }
}
