
const Discord = require('discord.js');
const client = new Discord.Client();
var Voiceid=0;
var Textid=0;

client.on('ready', () => {
    console.log(`Logged in as ${client.user.tag}!`);
    
  });
  

  client.on('guildMemberAdd', member => {
    var fs = require('fs');
    var s = ""; 
    var exist = false;
    fs.readFile('../speechModality/speechModality/bin/Debug/ptG.grxml', function(err, data) {
      var lines = data.toString().split('\n');
      lines.forEach(l => { 
        if(l == "      <item repeat=\"0-1\">"+member.displayName+"<tag>out.name=\""+member.displayName+"\"</tag></item>") {
          exist=true;
        }
      });

      if(!exist){
        lines.forEach(l => { 
          if(l == "      <!-- Keywords para identificar os utilizadores -->\r")
          {
           s += l + "\n      <item repeat=\"0-1\">"+member.displayName+"<tag>out.name=\""+member.displayName+"\"</tag></item>" + "\n";
          }
          else
          {
              s += l + "\n";
          }
        });
        fs.writeFile('../speechModality/speechModality/bin/Debug/ptG.grxml', s, function (err) {
          if (err) throw err;
          console.log('Saved!');
        });
      }      
    });
    
  });

  client.on('message', msg => {

    if(msg.channel.id=="643896744676950018"){

        //ADD VOICE CHANNEL
        if (msg.content === '!ADD_VOICE') {
          msg.delete();
          var server = msg.guild;
          var existVoice=false;

          while(!existVoice){
            if(server.channels.exists("name", "voz"+Voiceid.toString())){
              Voiceid +=1;
            }else{
              existVoice=true;
            }
          }

          if(client.channels)
          server.createChannel("voz"+Voiceid.toString(),'voice').then(
            (chan) =>{
              chan.setParent("637207215933095949");
              Voiceid +=1;
            }
          )
        }


        // ADD TEXT CHANNEL
        if (msg.content === '!ADD_TEXT') {
          msg.delete();
          var server = msg.guild;
          var existText=false;

          while(!existText){
            if(server.channels.exists("name", "texto"+Textid.toString())){
              Textid +=1;
            }else{
              existText=true;
            }
          }

          server.createChannel("texto"+Textid.toString(),'text').then(
            (chan) =>{
              chan.setParent("641735348795211796");
              Textid+=1;
            }
          )
        }


        if (msg.content === '!DELT') {
          msg.delete();
          var server = msg.guild;
          var channels = server.channels.array();

          var size = channels.length;
          console.log(size);
          for(var i=0; i<size; i++){
            if(channels[i].type==='text' && channels[i].name!='comandos'){
              var channel = server.channels.find("name", channels[i].name);
              if(channel){
                   channel.delete();
               }
               break;
            }
            
          }
        }

        if (msg.content === '!DELV') {
          msg.delete();
          var server = msg.guild;
          var channels = server.channels.array();

          var size = channels.length;
          console.log(size);
          for(var i=0; i<size; i++){
            if(channels[i].type==='voice' && channels[i].name!='comandos'){
              var channel = server.channels.find("name", channels[i].name);
              if(channel){
                   channel.delete();
               }
               break;
            }
            
          }
        }

        //ASSIGNMENT 3
        if (msg.content === '!MUTEALL') {
          msg.delete();
          var server = msg.guild;
          server.members.forEach(function(entry) { //IGNORE BOT
            if(entry.user.id!=637206643385565194){     
              var user1 =server.members.find('id', entry.user.id);
              user1.setMute(true);
            }
          });
        }

        //ASSIGNMENT 3
        if (msg.content === '!UNMUTEALL') {
          msg.delete();
          var server = msg.guild;
          server.members.forEach(function(entry) { //IGNORE BOT
            if(entry.user.id!=637206643385565194){
              var user1 =server.members.find('id', entry.user.id);
              user1.setMute(false);
            }
          });
        }

         //ASSIGNMENT 3
         if (msg.content === '!MOVEAFKALL') {
          msg.delete();
          var server = msg.guild;
          var channels = server.channels.array();
          var channel;
          var size = channels.length;
          console.log(size);
          for(var i=0; i<size; i++){
            if(channels[i].type==='voice'){
             channel = server.channels.find("name", 'AFK');
            }
            
          }
          
          server.members.forEach(function(entry) { //IGNORE BOT
            if(entry.user.id!=637206643385565194){
              var user1 =server.members.find('id', entry.user.id);
              user1.setVoiceChannel(channel);
              user1.setMute(true);
            }
          });
        }


        if (msg.content === '!UNMOVEAFKALL') {
          msg.delete();
          var server = msg.guild;
          var channels = server.channels.array();
          var channel;
          var size = channels.length;
          console.log(size);
          for(var i=0; i<size; i++){
            if(channels[i].type==='voice'){
             channel = server.channels.find("name", 'voz0');
            }
          }
          
          server.members.forEach(function(entry) { //IGNORE BOT
            if(entry.user.id!=637206643385565194){
              var user1 =server.members.find('id', entry.user.id);
              user1.setVoiceChannel(channel);
              user1.setMute(false);
            }
          });
        }

        


        if(msg.content.split(" ")){
          var msgAux = msg.content.split(" ");

          if (msgAux[0] === '!DEL') {
            msg.delete();
            var server = msg.guild;
            var channel = server.channels.find("name",  msgAux[1]);
              if(channel){
                  channel.delete();
              }
            }
            
            if (msgAux[0] === '!ADD_VOICE') {
              msg.delete();
              var server = msg.guild;

              server.createChannel(msgAux[1],'voice').then(
              (chan) =>{
                chan.setParent("637207215933095949");
              }
            )
          }

          if (msgAux[0] === '!ADD_TEXT') {
            msg.delete();
            var server = msg.guild;

            server.createChannel(msgAux[1],'text').then(
            (chan) =>{
              chan.setParent("641735348795211796");
            }
          )
        }

        if (msgAux[0] === '!MUTE') {
          msg.delete();
          var server = msg.guild;
          var try1 = server.members.find('nickname',msgAux[1]);
          var try2 = server.members.find('displayName',msgAux[1]);
          if(try1){
            try1.setMute(true);
            console.log(try1.mute);
          }else if(try2){
            try2.setMute(true);
            console.log(try2.mute);
          }else{
            msg.channel.sendMessage("UTILIZADOR NÃO ENCONTRADO!!")
          }
        }

        if (msgAux[0] === '!UNMUTE') {
          msg.delete();
          var server = msg.guild;
          var try1 = server.members.find('nickname',msgAux[1]);
          var try2 = server.members.find('displayName',msgAux[1]);
          if(try1){
            try1.setMute(false);
            console.log(try1.mute);
          }else if(try2){
            try2.setMute(false);
            console.log(try2.mute);
          }else{
            msg.channel.sendMessage("UTILIZADOR NÃO ENCONTRADO!!")
          }
         
          
        }

        if (msgAux[0] === '!KICK') {
          msg.delete();
          var server = msg.guild;
          var try1 = server.members.find('nickname',msgAux[1]);
          var try2 = server.members.find('displayName',msgAux[1]);
          if(try1){
            try1.kick("Foste expluso do canal.");
          }else if(try2){
            try2.kick("Foste expluso do canal.");
          }else{
            msg.channel.sendMessage("UTILIZADOR NÃO ENCONTRADO!!")
          }
         
          
        }

        if (msgAux[0] === '!BAN') {
          msg.delete();
          var server = msg.guild;
          var try1 = server.members.find('nickname',msgAux[1]);
          var try2 = server.members.find('displayName',msgAux[1]);
          if(try1){
            try1.ban("Foste banido do canal.", 1);
          }else if(try2){
            try2.ban("Foste banido do canal.", 1);
          }else{
            msg.channel.sendMessage("UTILIZADOR NÃO ENCONTRADO!!")
          }          
        }

        if (msgAux[0] === '!UNBAN') {
          msg.delete();
          var server = msg.guild;

          server.fetchBans()
            .then(banned => {
              console.log(banned);
              banned.forEach(l => {
                if(l.username === msgAux[1]){
                  msg.guild.unban(l);
                }
              });
            }).catch(console.error);
      }
      
      if (msgAux[0] === '!INVITE') {
        msg.delete();
        var server = msg.guild;
        var try1 = server.members.find('nickname',msgAux[1]);
        var try2 = server.members.find('displayName',msgAux[1]);
        var options = {
          maxAge: 3600,
          maxUses: 1
        };
        if(try1){
          var invite = msg.channel.createInvite(options).then(function(newInvite){
            try1.send("https://discord.gg/" + newInvite.code)
            });
        }else if(try2){
          var invite =  msg.channel.createInvite(options).then(function(newInvite){
            try2.send("Foste convidado para o canal\nhttps://discord.gg/" + newInvite.code)
            });
        }else{
          msg.channel.sendMessage("UTILIZADOR NÃO ENCONTRADO!!")
        }          
    }

    if (msgAux[0] === '!ADDROLEADMIN') {
      msg.delete();
      var server = msg.guild;
      var try1 = server.members.find('nickname',msgAux[1]);
      var try2 = server.members.find('displayName',msgAux[1]);
      if(try1){
        var role = server.roles.find(role => role.name === "administrador");
        try1.addRole(role);
      }else if(try2){
        var role = server.roles.find(role => role.name === "administrador");
        try2.addRole(role);
      }else{
        msg.channel.sendMessage("UTILIZADOR NÃO ENCONTRADO!!")
      }          
    }

  if (msgAux[0] === '!ADDROLEMOD') {
    msg.delete();
    var server = msg.guild;
    var try1 = server.members.find('nickname',msgAux[1]);
    var try2 = server.members.find('displayName',msgAux[1]);
    if(try1){
      var role = server.roles.find(role => role.name === "moderador");
      try1.addRole(role);
    }else if(try2){
      var role = server.roles.find(role => role.name === "moderador");
      try2.addRole(role);
    }else{
      msg.channel.sendMessage("UTILIZADOR NÃO ENCONTRADO!!")
    }          
  }

  if (msgAux[0] === '!DELROLEADMIN') {
    msg.delete();
    var server = msg.guild;
    var try1 = server.members.find('nickname',msgAux[1]);
    var try2 = server.members.find('displayName',msgAux[1]);
    if(try1){
      var role = server.roles.find(role => role.name === "administrador");
      try1.removeRole(role);
    }else if(try2){
      var role = server.roles.find(role => role.name === "administrador");
      try2.removeRole(role);
    }else{
      msg.channel.sendMessage("UTILIZADOR NÃO ENCONTRADO!!")
    }          
  }

if (msgAux[0] === '!DELROLEMOD') {
  msg.delete();
  var server = msg.guild;
  var try1 = server.members.find('nickname',msgAux[1]);
  var try2 = server.members.find('displayName',msgAux[1]);
  if(try1){
    var role = server.roles.find(role => role.name === "moderador");
    try1.removeRole(role);
  }else if(try2){
    var role = server.roles.find(role => role.name === "moderador");
    try2.removeRole(role);
  }else{
    msg.channel.sendMessage("UTILIZADOR NÃO ENCONTRADO!!")
  }          
}

    }
  }
  });
  
  client.login('NjM3MjA2NjQzMzg1NTY1MTk0.XgYV_g.ZDtAU6z4enBiG5lwGv1OmQphC8o');


