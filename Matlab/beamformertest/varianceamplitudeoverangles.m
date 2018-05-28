%Plot variance compared to angle
clc; clear all; close all;



folder='/Users/Hjem/Downloads/beamformertest2/beamformeronly';

%  folder='C:\Users\Nickl\Aalborg Universitet\OneDrive - Aalborg Universitet\beamformer\physicaltes\';
% audio_files=dir(fullfile(folder,'*.m'));

files800hz = dir(strcat(folder, '/**/*800hz_beamformer.wav'));
% dir('D:\Data\**\*.mat')
audio800 = cell(1,length(files800hz));
for k = 1:length(files800hz)

    disp(strcat(folder,files800hz(k).name));
    audio800{k} = audioread(strcat(folder,'/',files800hz(k).name));

end
variance800 = zeros(1, length(files800hz), 'double');
%    variance = cell(1,length(files));
for k = 1:length(files800hz)
   variance800(k) = (var(audio800{k})*10000);
end



files1600hz = dir(strcat(folder, '/**/*1600hz_beamformer.wav'));
% dir('D:\Data\**\*.mat')
audio1600 = cell(1,length(files1600hz));
for k = 1:length(files1600hz)

    disp(strcat(folder,files1600hz(k).name));
    audio1600{k} = audioread(strcat(folder,'/',files1600hz(k).name));

end
variance1600 = zeros(1, length(files1600hz), 'double');
%    variance = cell(1,length(files));
for k = 1:length(files1600hz)
   variance1600(k) = (var(audio1600{k})*10000);
end


meanSignalStrength800 = zeros(1, length(files800hz), 'double');


for k = 1:length(files800hz)
    LA = length(audio800{k});
    meanArr = zeros(1,LA,'double');
    tempcell = audio800{k}(1:LA,1);
    for t = 1:LA
        meanArr(1,t) = db(tempcell(t,1));
        if isfinite(meanArr(1,t)) == false
            meanArr(1,t) = -100;
        end
    end
    meanSignalStrength800(1,k) = mean(meanArr);
end


meanSignalStrength1600 = zeros(1, length(files1600hz), 'double');

for k = 1:length(files1600hz)
    LA = length(audio1600{k});
    meanArr = zeros(1,LA,'double');
    tempcell = audio1600{k}(1:LA,1);
    for t = 1:LA
        meanArr(1,t) = db(tempcell(t,1));
        if isfinite(meanArr(1,t)) == false
            meanArr(1,t) = -100;
        end
    end
    meanSignalStrength1600(1,k) = mean(meanArr);
end


anglegrid=linspace(180,360,length(files1600hz));
flipped800 = fliplr(meanSignalStrength800);
flipped1600 = fliplr(meanSignalStrength1600);
plot(anglegrid,flipped800,anglegrid,flipped1600)
xlim([0 180]);
title('Beamformer performance physical test');
str1 = [num2str(800), 'Hz']; 
str2 = [num2str(1600), 'Hz'];  
legend(str1, str2, 'Location', 'bestoutside');
xlabel('Degrees °')
ylabel('Decibel dB')

    