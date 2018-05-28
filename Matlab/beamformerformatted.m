%degree formatted
clc; clear all; close all;
anglegrid=linspace(0,360,360);


di = 0.075; %distance between michrophones
lisAngle = 45; %angle beamforming to
soundSpeed = 343; %speed of sound
arrSize = 200*8; %size of input signal size
sampleRate = 8000; %sample rate of input signal
ff = 200;

fileCount = 0;
 for c = 1:4
     for j = 1:4
        w1=[1; exp(-1i*(2*pi/arrSize)*sampleRate*c*ff/2*sind(lisAngle)*di*j/soundSpeed)]/2;
        w2=[1; exp(-1i*(2*pi/arrSize)*sampleRate*c*ff*sind(lisAngle)*di*j/soundSpeed)]/2;
        w3=[1; exp(-1i*(2*pi/arrSize)*sampleRate*c*ff*4*sind(lisAngle)*di*j/soundSpeed)]/2;
        w4=[1; exp(-1i*(2*pi/arrSize)*sampleRate*c*ff*8*sind(lisAngle)*di*j/soundSpeed)]/2;
            for tt=1:length(anglegrid)
                angle = sind(anglegrid(tt))*di*j/soundSpeed;
                d1 = [1;exp(-1i*(2*pi/(arrSize))*sampleRate*c*ff/2*angle)];
                d2 = [1;exp(-1i*(2*pi/(arrSize))*sampleRate*c*ff*angle)];
                d3 = [1;exp(-1i*(2*pi/(arrSize))*sampleRate*c*ff*4*angle)];
                d4 = [1;exp(-1i*(2*pi/(arrSize))*sampleRate*c*ff*8*angle)];
                H1(tt) = abs(w1'*d1);
                H2(tt) = abs(w2'*d2);
                H3(tt) = abs(w3'*d3);
                H4(tt) = abs(w4'*d4);
                fileCount = fileCount + 1;
            end

        plot(anglegrid,H1, '-.r*', anglegrid, H2, '-.g+', anglegrid, H3, '-.bo', anglegrid, H4, '-.cx','LineWidth', 1);
        ylim([0 1])
        xlim([0 369])
        str = strcat({'Sampling rate: '}, num2str(sampleRate*c),{', Length: '}, num2str(di*j), 'meters', {', Beamformed towards: '}, num2str(lisAngle), '°') ;
        title(str);
        str1 = [num2str(ff/2), 'Hz']; 
        str2 = [num2str(ff), 'Hz'];  
        str3 = [num2str(ff*4), 'Hz'];  
        str4 = [num2str(ff*8), 'Hz']; 
        legend(str1, str2, str3, str4, 'Location', 'bestoutside');
        set(gcf,'units','pixels','position',[10,10,1920,300])
        fileStr = strcat(num2str(sampleRate*c),'_',num2str(fix(di*j*100)),'_', num2str(lisAngle));
        print(fileStr, '-deps');
        pause;
     end
 end