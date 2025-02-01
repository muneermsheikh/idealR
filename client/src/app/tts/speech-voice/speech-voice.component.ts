import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { fromEvent, map, merge, Observable, Subscription, tap } from 'rxjs';
import { SpeechService } from '../speech.service';
import { PropertyName } from '../speech';

@Component({
  selector: 'app-speech-voice',
  templateUrl: './speech-voice.component.html',
  styleUrls: ['./speech-voice.component.css']
})
export class SpeechVoiceComponent implements OnInit, OnDestroy{

  @ViewChild('rate', { static: true, read: ElementRef })
  rate!: ElementRef<HTMLInputElement>;

  @ViewChild('pitch', { static: true, read: ElementRef })
  pitch!: ElementRef<HTMLInputElement>;

  @ViewChild('voices', { static: true, read: ElementRef })
  voiceDropdown!: ElementRef<HTMLSelectElement>;

  voices$!: Observable<SpeechSynthesisVoice[]>;
  subscription = new Subscription();

  constructor(private speechService: SpeechService) {}

  ngOnInit(): void {
    
      this.voices$ = fromEvent(speechSynthesis, 'voiceschanged').pipe(
        map(() => speechSynthesis.getVoices().filter((voice) => voice.lang.includes('en'))),
        tap((voices) => this.speechService.setVoices(voices)),
      );

      this.subscription.add(
        fromEvent(this.voiceDropdown.nativeElement, 'change')
          .pipe(tap(() => this.speechService.updateVoice(this.voiceDropdown.nativeElement.value)))
          .subscribe(),
      ); 

      this.subscription.add(
        merge(
          fromEvent(this.rate.nativeElement, 'change'),
          fromEvent(this.pitch.nativeElement, 'change')
        )
          .pipe(
            map((e) => e.target as HTMLInputElement),
            map((e) => ({ name: e.name as PropertyName, value: e.value })),
            tap((property) => this.speechService.updateSpeech(property)),
          )
          .subscribe(),
      );
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
  
}
