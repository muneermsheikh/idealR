import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { fromEvent, map, merge, Subject, Subscription, tap } from 'rxjs';
import { SpeechService } from '../speech.service';

@Component({
  selector: 'app-speech-text',
  templateUrl: './speech-text.component.html',
  styleUrls: ['./speech-text.component.css']
})
export class SpeechTextComponent implements OnInit, OnDestroy{

  @ViewChild('stop', { static: true, read: ElementRef })
  btnStop!: ElementRef<HTMLButtonElement>;

  @ViewChild('speak', { static: true, read: ElementRef })
  btnSpeak!: ElementRef<HTMLButtonElement>;

  textChanged$ = new Subject<void>();
  subscription = new Subscription();
  msg = 'Hello! I love JavaScript 👍';

  constructor(private speechService: SpeechService) {}

  ngOnInit(): void {
    this.speechService.updateSpeech({ name: 'text', value: this.msg });

    const btnStop$ = fromEvent(this.btnStop.nativeElement, 'click').pipe(
      map(() => false)
    );
    
    const btnSpeak$ = fromEvent(this.btnSpeak.nativeElement, 'click').pipe(
      map(() => true)
    );

    this.subscription.add(
      merge(btnStop$, btnSpeak$)
        .pipe(tap(() => this.speechService.updateSpeech({ name: 'text', value: this.msg })))
        .subscribe((startOver) => this.speechService.toggle(startOver)),
    );

    this.subscription.add(
      this.textChanged$.pipe(tap(() => this.speechService.updateSpeech({ name: 'text', value: this.msg }))).subscribe(),
    );
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
