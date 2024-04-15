let orderSidebar = document.getElementById("order-sidebar");
let orderSidebarBtn = document.getElementById("order-sidebar-btn");
let hideOrderElement = document.querySelector("#hideOrder");
let orderList = document.getElementsByClassName("order-list")[0];
let orderQuantity = document.getElementById("order-quantity");

const plus = "item-plus";
const minus = "item-minus";
const quantity = "item-quantity";
const price = "item-price";
const total = "order-total";
// Hide & Display order sidebar section
function DisplayOrder() {
	orderSidebar.style.display = "block";
	hideOrderElement.style.display = "block";
}

function HideOrder() {
	orderSidebar.style.display = "none";
	hideOrderElement.style.display = "none";
}

orderSidebarBtn.addEventListener("click", DisplayOrder);
hideOrderElement.addEventListener("click", HideOrder);

// Add to Cart section
const formatter = new Intl.NumberFormat("vi-VN", {
	style: "currency",
	currency: "VND"
});

UpdateOrder();
function AddToCartList(productName, productPrice) {
	let position = orderList.getElementsByClassName("order-item").length + 1;
	orderQuantity.innerHTML = position;
	let productDiv = `
    <li class="list-group my-4 order-item">
        <section class="text-decoration-none py-2 d-flex justify-content-between mb-0 text-white">
            <div class="d-flex flex-column item-content">
                <span>${position}. <span class="item-name">${productName}</span></span>
                <span>$ <span class="item-price">${productPrice}</span></span>
            </div>
            <div class="btn-group flex-shrink-1 item-action" role="group">
                <button type="button" class="item-minus btn btn-secondary py-0 px-1">
                    <i class="fas fa-fw fa-minus-square"></i>
                </button>
                <button class="btn px-0 btn-secondary item-quantity">1</button>
                <button type="button" class="item-plus btn btn-secondary py-0 px-1">
                    <i class="fas fa-fw fa-plus-square"></i>
                </button>
            </div>
        </section>
    </li>
    `;
	orderList.innerHTML += productDiv;
	UpdateOrder();
}

function AddToCart(event) {
	let target = event.target;
	let orderItems = orderList.getElementsByClassName("order-item");
	let isAddToCartButton = target.classList.contains("add-to-cart");
	let isIcon = target.classList.contains("fas");

	let addToCartBtn;
	if (isAddToCartButton) {
        addToCartBtn = target;
	}
    else addToCartBtn = target.parentElement;

	let productItem = addToCartBtn.parentElement.parentElement;
	// Get item information
	let productName = productItem.getElementsByClassName("product-name")[0].innerHTML;
	let productPrice = productItem.getElementsByClassName("product-price")[0].innerText;
	// Add item to Cart
	let isExist = false;
	for (let item of orderItems) {
		let itemName = item.getElementsByClassName("item-name")[0].innerText;
		if (productName === itemName) {
			isExist = true;
			break;
		}
	}
	if (!isExist) AddToCartList(productName, productPrice);
}

function ChangeQuantity(event) {
	let target = event.target;
	let isIcon = target.classList.contains("fas");
	let actionBtn;
	if (isIcon) {
		actionBtn = target.parentElement;
	} else actionBtn = target;

	let orderItem = actionBtn.parentElement.parentElement.parentElement;
	let itemQuantity = orderItem.getElementsByClassName(quantity)[0];
	let currentQuantity = itemQuantity.innerText;

	let isPlus = actionBtn.classList.contains(plus);
	let isMinus = actionBtn.classList.contains(minus);
	// Increase/Decrease quantity
	if (isPlus) currentQuantity++;
	if (isMinus) {
		currentQuantity--;
		if (currentQuantity == 0) orderItem.remove();
	}
	itemQuantity.innerText = currentQuantity;
	UpdateOrder();
}

function UpdateOrder() {
	let orderItems = orderList.getElementsByClassName("order-item");
	orderQuantity.innerHTML = orderItems.length;
	let totalPrice = 0;
	for (let item of orderItems) {
		let itemPrice = item.getElementsByClassName(price)[0].innerHTML;
		let itemQuantity = item.getElementsByClassName(quantity)[0].innerHTML;
		totalPrice += itemPrice * itemQuantity;
	}
	let totalDivs = document.getElementsByClassName(total);
	for (let item of totalDivs) item.innerHTML = formatter.format(totalPrice);
}
