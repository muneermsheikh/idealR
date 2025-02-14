import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { IAudioMessageDto } from 'src/app/_dtos/hr/audioMessageDto';
import { Pagination } from 'src/app/_models/pagination';
import { audioMessageParams } from 'src/app/_models/params/audioMessageParams';
import { User } from 'src/app/_models/user';
import { AudioService } from 'src/app/_services/audio.service';

@Component({
  selector: 'app-audio',
  templateUrl: './audio.component.html',
  styleUrls: ['./audio.component.css']
})
export class AudioComponent implements OnInit{

  user?: User;
    returnUrl = '';
  
    audioDtos: IAudioMessageDto[]=[];
    
    totalCount = 0;
    aParams = new audioMessageParams();
    
    pagination: Pagination | undefined;
    
    isPrintPDF = false;
    printtitle = "";
    
    constructor(private service: AudioService, private router: Router) {
      service.setParams(this.aParams);
    }
  
    ngOnInit(): void {
      this.loadAudioLists(false);
    }
  
    loadAudioLists(cached: boolean = true) {
        var params = this.service.getParams();
        this.service.setParams(this.aParams);
  
        this.service.getAudioMessagesPaged(cached)?.subscribe({
          next: response => {
            if(response !== undefined && response !== null) {
              this.audioDtos = response.result;
              this.totalCount = response?.count;
              this.pagination = response.pagination;
            } 
          },
          error: error => console.log(error)
        })
        console.log('audiodtos', this.audioDtos);
    }
  
    close() {
      this.router.navigateByUrl(this.returnUrl);
    }
  
    
    onPageChanged(event: any){
      const params = this.service.getParams();
      if (params!.pageNumber !== event.page) {
        params!.pageNumber = event.page;
        this.service.setParams(params!);
  
        this.loadAudioLists();
      }
    }
    
    setParameters() {
      var params = new audioMessageParams();
      this.service.setParams(params);
      this.loadAudioLists();
    }
}
