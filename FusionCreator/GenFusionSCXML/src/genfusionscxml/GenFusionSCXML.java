/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package genfusionscxml;

import java.io.IOException;
import scxmlgen.Fusion.FusionGenerator;
import scxmlgen.Modalities.Output;
import scxmlgen.Modalities.Speech;
import scxmlgen.Modalities.Gesture;

/**
 *
 * @author nunof
 */
public class GenFusionSCXML {

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) throws IOException {

    FusionGenerator fg = new FusionGenerator();
        
    fg.Redundancy(Speech.NEWS, Gesture.BALL, Output.NEWS);
    fg.Redundancy(Speech.LEAVE, Gesture.LEAVE, Output.LEAVE);
    fg.Redundancy(Speech.JOIN, Gesture.INVITE, Output.JOIN);
    
    fg.Complementary(Gesture.UMBRELLA, Speech.AVEIRO, Output.UMBRELLA_AVEIRO);
    fg.Complementary(Gesture.UMBRELLA, Speech.PORTO, Output.UMBRELLA_PORTO);
    fg.Complementary(Gesture.MUTE, Speech.JOAO, Output.MUTE_JOAO);
    fg.Complementary(Gesture.MUTE, Speech.ANDRE, Output.MUTE_ANDRE);
    fg.Complementary(Gesture.MUTE, Speech.TODOS, Output.MUTE_TODOS);
    fg.Complementary(Gesture.UNMUTE, Speech.JOAO, Output.UNMUTE_JOAO);
    fg.Complementary(Gesture.UNMUTE, Speech.ANDRE, Output.UNMUTE_ANDRE);
    fg.Complementary(Gesture.MUTE, Speech.TODOS, Output.UNMUTE_TODOS);
    
    fg.Build("fusion.scxml");
        
        
    }
    
}
