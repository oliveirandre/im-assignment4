package scxmlgen.Modalities;

import scxmlgen.interfaces.IModality;

/**
 *
 * @author nunof
 */
public enum Gesture implements IModality{

    MUTE("[3][muteR]",3000),
    UNMUTE("[5][unmuteR]",3000),
    LEAVE("[2][leave]",3000),
    INVITE("[1][invite]",3000),
    UMBRELLA("[4][tempo]",3000),
    BALL("[0][bola]",3000);
    
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
