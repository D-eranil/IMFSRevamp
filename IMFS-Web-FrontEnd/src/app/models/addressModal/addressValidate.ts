export class AddressAutoComplete {
	geoCodeEnable:boolean;
	skipAddressLookup:boolean;
	addressInfo:AddressInfo;
}
export class AddressInfo{
	addressLine1:string;
	addressLine2:string;
	city:string;
	companyName:string;
	country:string;
	postalCode:string;
	state:string;
}
export class AddressValidateTokenResponse {
	addressCompare:{
		addressInfo: {
            contact: number;
            companyName: string;
            addressLine1: string;
            addressLine2: string;
            addressLine3: string;
            city: string,
            state: string,
            country: string,
            postalCode: number;
        },
		suggestedAddress:{
			contact: number;
            companyName: string;
            addressLine1: string;
            addressLine2: string;
            addressLine3: string;
            city: string,
            state: string,
            country: string,
            postalCode: number;
		},
		addressInputAnalysis:{
			addressInputCorrect	:string;
			primaryInfo	:any;
			generalAddress:boolean;
			noDelivery:boolean;
			rdiStatus:null;
			vacantAddress:null;
			validPostBox:boolean;
			zipCodePrecision:null;
			addressInputFeedback:any;
		}
		}
		match:{
		addressQuality:	any;
		verificationQuality:any;
		preProcessScore	:any;
		matchScore:20
		promptThreshold	:null,
		geoCode:{
			geoAccuracyMatch:any;	
			latitude:any;
			longitude:any;
			coordinateLicense:null
			}	
		}
		
			
}
