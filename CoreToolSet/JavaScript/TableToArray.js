var selTable = "//table[@id='softwarelist']";

/*
	var tableHeader = document.evaluate(`${selTable}//thead//tr`,document,null,XPathResult.ORDERED_NODE_SNAPSHOT_TYPE,null);
	var tableBody = document.evaluate(`${selTable}//tbody//tr`,document,null,XPathResult.ORDERED_NODE_SNAPSHOT_TYPE,null);
	var tableFoot = document.evaluate(`${selTable}//tfoot//tr`,document,null,XPathResult.ORDERED_NODE_SNAPSHOT_TYPE,null);
*/

function IterateThruSnapShots(inElement){
	if (inElement == null){
		return null;
	}
	else{
		
	}
	
}
	
function GetHeader(xpath){
	try {
		let outElement = document.evaluate(`${xpath}//thead//tr`,document,null,XPathResult.ORDERED_NODE_SNAPSHOT_TYPE,null);
		return outElement;
	}
	catch (e){
		return null;
	}
}

function GetBody(xpath){
	try{
		let outElement = document.evaluate(`${xpath}//tbody//tr`,document,null,XPathResult.ORDERED_NODE_SNAPSHOT_TYPE,null);
		return outElement;
	}
	catch (e){
		return null;
	}

}

function GetFooter(xpath){
	try {
		let outElement = document.evaluate(`${xpath}//tfoot//tr`,document,null,XPathResult.ORDERED_NODE_SNAPSHOT_TYPE,null);
		return outElement;
	}
	catch (e){
		return null;
	}

}





