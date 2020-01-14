package scxmlgen.Modalities;

import scxmlgen.interfaces.IModality;

/**
 *
 * @author nunof
 */
public enum Gesture implements IModality{

    MUTE("[3][muteR]",1500),
    UNMUTE("[5][unmute1_Right]",1500),
    LEAVE("[2][leave]",1500),
    INVITE("[1][invite]",1500),
    UMBRELLA("[4][tempo]",1500),
    BALL("[0][bola]",1500);
    
    private String event;
    private int timeout;


    Gesture(String m, int time) {
        event=m;
        timeout=time;
    }

    @Override
    public int getTimeOut() {
        return timeout;
    }

    @Override
    public String getEventName() {
        //return getModalityName()+"."+event;
        return event;
    }

    @Override
    public String getEvName() {
        return getModalityName().toLowerCase()+event.toLowerCase();
    }
    
}
