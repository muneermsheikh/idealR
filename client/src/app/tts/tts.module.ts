import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SpeechSynthesisComponent } from './speech-synthesis/speech-synthesis.component';
import { SpeechTextComponent } from './speech-text/speech-text.component';
import { SpeechVoiceComponent } from './speech-voice/speech-voice.component';
import { FormsModule } from '@angular/forms';



@NgModule({
  declarations: [
    SpeechSynthesisComponent,
    SpeechTextComponent,
    SpeechVoiceComponent
  ],
  imports: [
    CommonModule,
    FormsModule
  ],
  exports: [
    SpeechSynthesisComponent
  ]
})
export class TtsModule { }
