import { ResolveFn } from '@angular/router';
import { Member } from '../_models/member';
import { MemberService } from '../_services/member.service';
import { inject } from '@angular/core';

export const memberDetailedResolver: ResolveFn<Member> = (route, state) => {

  const memberService = inject(MemberService);

  return memberService.getMember(route.paramMap.get('username')!);
};
