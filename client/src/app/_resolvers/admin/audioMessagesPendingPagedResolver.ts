import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IAudioMessageDto } from "src/app/_dtos/hr/audioMessageDto";
import { PaginatedResult } from "src/app/_models/pagination";
import { audioMessageParams } from "src/app/_models/params/audioMessageParams";
import { AudioService } from "src/app/_services/audio.service";

export const AudioMessagesPendingPagedResolver: ResolveFn<PaginatedResult<IAudioMessageDto[]|null>> = (
  ) => {
    var params = new audioMessageParams();
    inject(AudioService).setParams(params);
    return inject(AudioService).getAudioMessagesPaged(false);
  };